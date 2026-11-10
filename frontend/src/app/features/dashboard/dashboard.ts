import { CurrencyPipe, DatePipe, PercentPipe } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CategoriaService } from '../../core/api/categoria.service';
import { MovimentacaoService } from '../../core/api/movimentacao.service';
import { ResumoService } from '../../core/api/resumo.service';
import { AuthService } from '../../core/auth/auth.service';
import { Movimentacao, ResumoMensal } from '../../core/models/api.models';
import { CORES_CATEGORIAS, corCategoria, iconeCategoria } from '../../shared/categoria-visual';
import { ItemMovimentacao } from '../../shared/item-movimentacao';
import { primeiroNome } from '../../shared/texto';

interface FatiaGrafico {
  nome: string;
  valor: number;
  percentual: number;
  cor: string;
  icone: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [
    CurrencyPipe,
    DatePipe,
    PercentPipe,
    RouterLink,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    MatTooltipModule,
    ItemMovimentacao,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  private readonly resumoService = inject(ResumoService);
  private readonly movimentacaoService = inject(MovimentacaoService);
  private readonly categoriaService = inject(CategoriaService);

  private readonly auth = inject(AuthService);

  protected readonly nomeUsuario = computed(() => primeiroNome(this.auth.usuario()?.nome));

  /** Primeiro dia do mês exibido */
  protected readonly mes = signal(primeiroDiaDoMes(new Date()));
  protected readonly resumo = signal<ResumoMensal | null>(null);
  protected readonly recentes = signal<Movimentacao[]>([]);
  private readonly nomesCategorias = signal(new Map<string, string>());
  protected readonly carregando = signal(false);

  protected readonly ehMesAtual = computed(
    () => this.mes().getTime() === primeiroDiaDoMes(new Date()).getTime(),
  );

  protected readonly semDados = computed(() => {
    const r = this.resumo();
    return !!r && r.totalEntradas === 0 && r.totalSaidas === 0;
  });

  /** Quanto das entradas sobrou (0 a 1). Null se não houve entradas */
  protected readonly taxaEconomia = computed(() => {
    const r = this.resumo();
    if (!r || r.totalEntradas === 0) return null;
    return Math.max(r.saldo / r.totalEntradas, 0);
  });

  /** Fatias do gráfico de rosca: as 5 maiores categorias de saída + "Outras" */
  protected readonly fatias = computed<FatiaGrafico[]>(() => {
    const r = this.resumo();
    if (!r || r.totalSaidas === 0) return [];

    const ordenadas = r.categorias
      .filter((c) => c.totalSaidas > 0)
      .sort((a, b) => b.totalSaidas - a.totalSaidas);

    const principais = ordenadas.slice(0, 5).map((c) => ({ nome: c.nome, valor: c.totalSaidas }));
    const resto = ordenadas.slice(5).reduce((soma, c) => soma + c.totalSaidas, 0);
    if (resto > 0) principais.push({ nome: 'Outras', valor: resto });

    // Cada fatia com uma cor diferente: se a cor da categoria já foi usada, pega a próxima livre
    const usadas = new Set<string>();
    return principais.map((f) => {
      let cor = f.nome === 'Outras' ? '#9a97a8' : corCategoria(f.nome);
      if (usadas.has(cor)) cor = CORES_CATEGORIAS.find((c) => !usadas.has(c)) ?? cor;
      usadas.add(cor);
      return { ...f, percentual: f.valor / r.totalSaidas, cor, icone: iconeCategoria(f.nome) };
    });
  });

  /** CSS do gráfico de rosca (conic-gradient com uma faixa por categoria) */
  protected readonly gradienteRosca = computed(() => {
    let acumulado = 0;
    const partes = this.fatias().map((f) => {
      const inicio = acumulado * 100;
      acumulado += f.percentual;
      return `${f.cor} ${inicio}% ${acumulado * 100}%`;
    });
    return partes.length ? `conic-gradient(${partes.join(', ')})` : 'none';
  });

  ngOnInit(): void {
    this.carregar();
  }

  protected nomeCategoria(id: string | null): string | null {
    return id ? (this.nomesCategorias().get(id) ?? null) : null;
  }

  protected mudarMes(delta: number): void {
    const atual = this.mes();
    this.mes.set(new Date(atual.getFullYear(), atual.getMonth() + delta, 1));
    this.carregar();
  }

  private carregar(): void {
    const inicio = this.mes();
    const fim = new Date(inicio.getFullYear(), inicio.getMonth() + 1, 0);
    this.carregando.set(true);

    forkJoin({
      resumo: this.resumoService.mensal(inicio.getFullYear(), inicio.getMonth() + 1),
      movimentacoes: this.movimentacaoService.listar(inicio, fim),
      categorias: this.categoriaService.listar(),
    }).subscribe({
      next: ({ resumo, movimentacoes, categorias }) => {
        this.resumo.set(resumo);
        this.recentes.set(movimentacoes.slice(0, 6));
        this.nomesCategorias.set(new Map(categorias.map((c) => [c.categoriaId, c.nome])));
        this.carregando.set(false);
      },
      error: () => this.carregando.set(false),
    });
  }
}

function primeiroDiaDoMes(data: Date): Date {
  return new Date(data.getFullYear(), data.getMonth(), 1);
}
