import { inject, Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

/** Mensagens rápidas no rodapé da tela (snackbar do Material) */
@Injectable({ providedIn: 'root' })
export class NotificacaoService {
  private readonly snackBar = inject(MatSnackBar);

  sucesso(mensagem: string): void {
    this.snackBar.open(mensagem, 'OK', { duration: 3000 });
  }

  erro(mensagem: string): void {
    this.snackBar.open(mensagem, 'Fechar', { duration: 6000 });
  }
}
