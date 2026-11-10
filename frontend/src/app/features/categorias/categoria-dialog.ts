import { Component, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface DadosCategoriaDialog {
  /** Nome atual (edição) ou vazio (criação) */
  nome?: string;
}

/** Pede o nome da categoria. Fecha com o nome digitado ou undefined (cancelou) */
@Component({
  selector: 'app-categoria-dialog',
  imports: [ReactiveFormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>{{ edicao ? 'Renomear categoria' : 'Nova categoria' }}</h2>
    <form [formGroup]="form" (ngSubmit)="salvar()">
      <mat-dialog-content>
        <mat-form-field class="largura-total">
          <mat-label>Nome</mat-label>
          <input matInput formControlName="nome" maxlength="50" cdkFocusInitial />
          <mat-hint align="end">{{ nome.value.length }}/50</mat-hint>
          <mat-error>Informe um nome de até 50 caracteres</mat-error>
        </mat-form-field>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button type="button" mat-dialog-close>Cancelar</button>
        <button mat-flat-button type="submit">Salvar</button>
      </mat-dialog-actions>
    </form>
  `,
})
export class CategoriaDialog {
  private readonly dados = inject<DadosCategoriaDialog>(MAT_DIALOG_DATA);
  private readonly dialogRef = inject(MatDialogRef<CategoriaDialog, string>);

  protected readonly edicao = !!this.dados.nome;
  protected readonly nome = new FormControl(this.dados.nome ?? '', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(50), Validators.pattern(/\S/)],
  });

  // O (ngSubmit) só funciona com [formGroup] no <form>; sem ele o Enter recarregaria a página
  protected readonly form = new FormGroup({ nome: this.nome });

  protected salvar(): void {
    if (this.nome.invalid) {
      this.nome.markAsTouched();
      return;
    }
    this.dialogRef.close(this.nome.value.trim());
  }
}
