import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { filter, switchMap } from 'rxjs';
import { CategoriaService } from '../../core/api/categoria.service';
import { MovimentacaoService } from '../../core/api/movimentacao.service';
import { Categoria, Movimentacao, TipoMovimentacao } from '../../core/models/api.models';
import { NotificacaoService } from '../../core/notificacao.service';
import { confirmar } from '../../shared/confirmacao-dialog';
import { ItemMovimentacao } from '../../shared/item-movimentacao';
import { DadosMovimentacaoDialog, MovimentacaoDialog } from './movimentacao-dialog';

type Periodo = 'mes' | 'anterior' | 'tudo' | 'personalizado';

interface GrupoDia {
  chave: string;
  data: Date;
  itens: Movimentacao[];
  total: number;
}

@Component({
  selector: 'app-movimentacoes',
  imports: [
    CurrencyPipe,
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatProgressBarModule,
    ItemMovimentacao,
  ],
  templateUrl: './movimentacoes.html',
  styleUrl: './movimentacoes.scss',
})
export class Movimentacoes implements OnInit {
  private readonly movimentacaoService = inject(MovimentacaoService);
  private readonly categoriaService = inject(CategoriaService);
  private readonly notificacao = inject(NotificacaoService);
  private readonly dialog = inject(MatDialog);

  protected readonly movimentacoes = signal<Movimentacao[]>([]);
  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly carregando = signal(false);
  protected readonly periodo = signal<Periodo>('mes');
  protected readonly busca = signal('');

  /** Intervalo de datas do seletor (preenchido conforme o período escolhido) */
  protected readonly intervalo = new FormGroup({
    inicio: new FormControl<Date | null>(null),
    fim: new FormControl<Date | null>(null),
  });

  /** A API devolve só o id da categoria; montamos um mapa id → nome para exibir */
  private readonly nomesCategorias = computed(
    () => new Map(this.categorias().map((c) => [c.categoriaId, c.nome])),
  );

  /** Lista após a busca por texto (descrição ou categoria) */
  protected readonly filtradas = computed(() => {
    const termo = this.busca().trim().toLowerCase();
    if (!termo) return this.movimentacoes();
    return this.movimentacoes().filter((m) =>
      `${m.descricao ?? ''} ${this.nomeCategoria(m.categoriaId) ?? ''}`.toLowerCase().includes(termo),
    );
  });

  protected readonly totais = computed(() => {
    let entradas = 0;
    let saidas = 0;
    for (const m of this.filtradas()) {
      if (m.tipo === TipoMovimentacao.Entrada) entradas += m.valor;
      else saidas += m.valor;
    }
    return { entradas, saidas, saldo: entradas - saidas };
  });

  /** Agrupa por dia (a API já devolve da mais recente para a mais antiga) */
  protected readonly grupos = computed<GrupoDia[]>(() => {
    const grupos = new Map<string, GrupoDia>();
    for (const m of this.filtradas()) {
      const data = new Date(m.dataTransacao);
      const chave = data.toDateString();
      let grupo = grupos.get(chave);
      if (!grupo) {
        grupo = { chave, data, itens: [], total: 0 };
        grupos.set(chave, grupo);
      }
      grupo.itens.push(m);
      grupo.total += m.tipo === TipoMovimentacao.Entrada ? m.valor : -m.valor;
    }
    return [...grupos.values()];
  });

  ngOnInit(): void {
    this.categoriaService
      .listar()
      .subscribe((c) => this.categorias.set(c.sort((a, b) => a.nome.localeCompare(b.nome))));
    this.escolherPeriodo('mes');
  }

  protected nomeCategoria(id: string | null): string | null {
    return id ? (this.nomesCategorias().get(id) ?? null) : null;
  }

  protected rotuloDia(data: Date): string | null {
    const hoje = new Date();
    const ontem = new Date(hoje.getFullYear(), hoje.getMonth(), hoje.getDate() - 1);
    if (data.toDateString() === hoje.toDateString()) return 'Hoje';
    if (data.toDateString() === ontem.toDateString()) return 'Ontem';
    return null;
  }

  protected escolherPeriodo(periodo: Periodo): void {
    this.periodo.set(periodo);
    const hoje = new Date();

    if (periodo === 'mes') {
      this.intervalo.setValue({
        inicio: new Date(hoje.getFullYear(), hoje.getMonth(), 1),
        fim: new Date(hoje.getFullYear(), hoje.getMonth() + 1, 0),
      });
    } else if (periodo === 'anterior') {
      this.intervalo.setValue({
        inicio: new Date(hoje.getFullYear(), hoje.getMonth() - 1, 1),
        fim: new Date(hoje.getFullYear(), hoje.getMonth(), 0),
      });
    } else if (periodo === 'tudo') {
      this.intervalo.setValue({ inicio: null, fim: null });
    }

    if (periodo !== 'personalizado') this.carregar();
  }

  /** Chamado quando o usuário fecha o seletor de datas */
  protected aoEscolherIntervalo(): void {
    const { inicio, fim } = this.intervalo.getRawValue();
    if (!inicio || !fim) return;
    this.periodo.set('personalizado');
    this.carregar();
  }

  protected nova(): void {
    this.dialog
      .open<MovimentacaoDialog, DadosMovimentacaoDialog, boolean>(MovimentacaoDialog, {
        data: { categorias: this.categorias() },
        width: '480px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .pipe(filter(Boolean))
      .subscribe(() => {
        this.notificacao.sucesso('Movimentação registrada.');
        this.carregar();
      });
  }

  protected excluir(m: Movimentacao): void {
    const nome = m.descricao || this.nomeCategoria(m.categoriaId) || 'esta movimentação';
    confirmar(this.dialog, {
      titulo: 'Excluir movimentação',
      mensagem: `Excluir "${nome}"? Essa ação não pode ser desfeita.`,
      textoConfirmar: 'Excluir',
    })
      .pipe(
        filter(Boolean),
        switchMap(() => this.movimentacaoService.excluir(m.movimentacaoId)),
      )
      .subscribe(() => {
        this.movimentacoes.update((lista) =>
          lista.filter((x) => x.movimentacaoId !== m.movimentacaoId),
        );
        this.notificacao.sucesso('Movimentação excluída.');
      });
  }

  private carregar(): void {
    const { inicio, fim } = this.intervalo.getRawValue();
    this.carregando.set(true);
    this.movimentacaoService.listar(inicio, fim).subscribe({
      next: (lista) => {
        this.movimentacoes.set(lista);
        this.carregando.set(false);
      },
      error: () => this.carregando.set(false),
    });
  }
}
