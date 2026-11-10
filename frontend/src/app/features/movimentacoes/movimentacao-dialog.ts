import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MovimentacaoService } from '../../core/api/movimentacao.service';
import { Categoria, TipoMovimentacao } from '../../core/models/api.models';

export interface DadosMovimentacaoDialog {
  categorias: Categoria[];
}

/** Formulário de nova movimentação. Fecha com true quando salvou */
@Component({
  selector: 'app-movimentacao-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule,
  ],
  templateUrl: './movimentacao-dialog.html',
  styleUrl: './movimentacao-dialog.scss',
})
export class MovimentacaoDialog {
  private readonly movimentacaoService = inject(MovimentacaoService);
  private readonly dialogRef = inject(MatDialogRef<MovimentacaoDialog, boolean>);

  protected readonly categorias = inject<DadosMovimentacaoDialog>(MAT_DIALOG_DATA).categorias;
  protected readonly Tipo = TipoMovimentacao;
  protected readonly hoje = new Date();
  protected readonly salvando = signal(false);

  protected readonly form = inject(NonNullableFormBuilder).group({
    tipo: [TipoMovimentacao.Saida, Validators.required],
    valor: [null as number | null, [Validators.required, Validators.min(0.01)]],
    data: [new Date(), Validators.required],
    categoriaId: [null as string | null],
    descricao: ['', Validators.maxLength(255)],
  });

  protected salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { tipo, valor, data, categoriaId, descricao } = this.form.getRawValue();
    this.salvando.set(true);

    this.movimentacaoService
      .criar({
        tipo,
        valor: Math.round(valor! * 100) / 100,
        dataOperacao: comHorarioAtual(data).toISOString(),
        categoriaId,
        descricao: descricao.trim() || null,
      })
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: () => this.salvando.set(false),
      });
  }
}

/** O datepicker devolve meia-noite; usamos o horário atual para manter a ordem dos lançamentos do dia */
function comHorarioAtual(data: Date): Date {
  const agora = new Date();
  return new Date(
    data.getFullYear(),
    data.getMonth(),
    data.getDate(),
    agora.getHours(),
    agora.getMinutes(),
    agora.getSeconds(),
  );
}
