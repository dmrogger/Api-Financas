import { Component, input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

/**
 * Moldura das telas de login e cadastro: painel de apresentação à esquerda
 * e o formulário (conteúdo projetado com <ng-content>) à direita.
 */
@Component({
  selector: 'app-auth-layout',
  imports: [MatIconModule],
  templateUrl: './auth-layout.html',
  styleUrl: './auth-layout.scss',
})
export class AuthLayout {
  readonly titulo = input.required<string>();
  readonly subtitulo = input<string>('');

  protected readonly ano = new Date().getFullYear();
}
