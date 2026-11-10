import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialog, MatDialogModule } from '@angular/material/dialog';
import { Observable } from 'rxjs';

export interface DadosConfirmacao {
  titulo: string;
  mensagem: string;
  textoConfirmar?: string;
}

/** Diálogo genérico "Tem certeza?". Fecha com true (confirmou) ou undefined */
@Component({
  selector: 'app-confirmacao-dialog',
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ dados.titulo }}</h2>
    <mat-dialog-content>{{ dados.mensagem }}</mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancelar</button>
      <button mat-flat-button [mat-dialog-close]="true">
        {{ dados.textoConfirmar ?? 'Confirmar' }}
      </button>
    </mat-dialog-actions>
  `,
})
export class ConfirmacaoDialog {
  protected readonly dados = inject<DadosConfirmacao>(MAT_DIALOG_DATA);
}

/** Atalho: abre o diálogo e devolve um Observable que emite true se o usuário confirmar */
export function confirmar(dialog: MatDialog, dados: DadosConfirmacao): Observable<boolean | undefined> {
  return dialog.open(ConfirmacaoDialog, { data: dados, width: '420px' }).afterClosed();
}
