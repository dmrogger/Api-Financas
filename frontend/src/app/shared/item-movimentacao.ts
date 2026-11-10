import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, computed, input, output } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { Movimentacao, TipoMovimentacao } from '../core/models/api.models';
import { corCategoria, iconeCategoria } from './categoria-visual';

/** Uma linha de movimentação: ícone da categoria, descrição, data e valor */
@Component({
  selector: 'app-item-movimentacao',
  imports: [CurrencyPipe, DatePipe, MatIconModule, MatButtonModule, MatTooltipModule],
  template: `
    <span class="icone-circulo" [style.--cor]="cor()">
      <mat-icon>{{ icone() }}</mat-icon>
    </span>

    <div class="texto">
      <strong>{{ movimentacao().descricao || nomeCategoria() || 'Sem descrição' }}</strong>
      <span>
        {{ nomeCategoria() || 'Sem categoria' }}
        @if (mostrarData()) {
          · {{ movimentacao().dataTransacao | date: 'dd MMM' }}
        }
      </span>
    </div>

    <span class="valor numero" [class.valor-entrada]="entrada()" [class.valor-saida]="!entrada()">
      {{ entrada() ? '+' : '−' }} {{ movimentacao().valor | currency }}
    </span>

    @if (podeExcluir()) {
      <button
        mat-icon-button
        class="excluir"
        (click)="excluir.emit()"
        matTooltip="Excluir"
        aria-label="Excluir movimentação"
      >
        <mat-icon>delete_outline</mat-icon>
      </button>
    }
  `,
  styles: `
    :host {
      display: flex;
      align-items: center;
      gap: 14px;
      padding: 12px 8px;
      border-radius: 14px;
      transition: background 0.15s;
    }
    :host(:hover) {
      background: var(--superficie-2);
    }
    .texto {
      flex: 1;
      min-width: 0;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }
    .texto strong,
    .texto span {
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    .texto strong {
      font-weight: 600;
    }
    .texto span {
      font-size: 12px;
      color: var(--texto-suave);
    }
    .valor {
      font-weight: 600;
      white-space: nowrap;
    }
    .excluir {
      color: var(--texto-suave);
      opacity: 0.4;
    }
    :host(:hover) .excluir {
      opacity: 1;
    }
  `,
})
export class ItemMovimentacao {
  readonly movimentacao = input.required<Movimentacao>();
  readonly nomeCategoria = input<string | null>(null);
  readonly mostrarData = input(true);
  readonly podeExcluir = input(false);
  readonly excluir = output<void>();

  protected readonly entrada = computed(
    () => this.movimentacao().tipo === TipoMovimentacao.Entrada,
  );
  protected readonly cor = computed(() => corCategoria(this.nomeCategoria()));
  protected readonly icone = computed(() => iconeCategoria(this.nomeCategoria()));
}
