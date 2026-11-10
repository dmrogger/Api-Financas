import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { filter, switchMap } from 'rxjs';
import { CategoriaService } from '../../core/api/categoria.service';
import { Categoria } from '../../core/models/api.models';
import { NotificacaoService } from '../../core/notificacao.service';
import { corCategoria, iconeCategoria } from '../../shared/categoria-visual';
import { confirmar } from '../../shared/confirmacao-dialog';
import { CategoriaDialog, DadosCategoriaDialog } from './categoria-dialog';

@Component({
  selector: 'app-categorias',
  imports: [MatButtonModule, MatIconModule, MatProgressBarModule, MatTooltipModule],
  templateUrl: './categorias.html',
  styleUrl: './categorias.scss',
})
export class Categorias implements OnInit {
  private readonly categoriaService = inject(CategoriaService);
  private readonly notificacao = inject(NotificacaoService);
  private readonly dialog = inject(MatDialog);

  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly carregando = signal(false);

  protected readonly minhas = computed(() =>
    this.categorias()
      .filter((c) => !c.padrao)
      .sort((a, b) => a.nome.localeCompare(b.nome)),
  );
  protected readonly padrao = computed(() =>
    this.categorias()
      .filter((c) => c.padrao)
      .sort((a, b) => a.nome.localeCompare(b.nome)),
  );

  protected readonly cor = corCategoria;
  protected readonly icone = iconeCategoria;

  ngOnInit(): void {
    this.carregar();
  }

  protected nova(): void {
    this.abrirDialogo({})
      .pipe(switchMap((nome) => this.categoriaService.criar(nome)))
      .subscribe(() => {
        this.notificacao.sucesso('Categoria criada.');
        this.carregar();
      });
  }

  protected renomear(c: Categoria): void {
    this.abrirDialogo({ nome: c.nome })
      .pipe(
        filter((nome) => nome !== c.nome),
        switchMap((nome) => this.categoriaService.editar(c.categoriaId, nome)),
      )
      .subscribe(() => {
        this.notificacao.sucesso('Categoria renomeada.');
        this.carregar();
      });
  }

  protected excluir(c: Categoria): void {
    confirmar(this.dialog, {
      titulo: 'Excluir categoria',
      mensagem: `Excluir "${c.nome}"? As movimentações dela ficarão sem categoria.`,
      textoConfirmar: 'Excluir',
    })
      .pipe(
        filter(Boolean),
        switchMap(() => this.categoriaService.excluir(c.categoriaId)),
      )
      .subscribe(() => {
        this.categorias.update((lista) => lista.filter((x) => x.categoriaId !== c.categoriaId));
        this.notificacao.sucesso('Categoria excluída.');
      });
  }

  private carregar(): void {
    this.carregando.set(true);
    this.categoriaService.listar().subscribe({
      next: (lista) => {
        this.categorias.set(lista);
        this.carregando.set(false);
      },
      error: () => this.carregando.set(false),
    });
  }

  private abrirDialogo(dados: DadosCategoriaDialog) {
    return this.dialog
      .open<CategoriaDialog, DadosCategoriaDialog, string>(CategoriaDialog, {
        data: dados,
        width: '420px',
        maxWidth: '95vw',
      })
      .afterClosed()
      .pipe(filter((nome): nome is string => !!nome));
  }
}
