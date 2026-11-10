import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router, RouterLink } from '@angular/router';
import { switchMap } from 'rxjs';
import { UsuarioService } from '../../../core/api/usuario.service';
import { AuthService } from '../../../core/auth/auth.service';
import { NotificacaoService } from '../../../core/notificacao.service';
import { DICA_SENHA, senhasIguais, validadorSenha } from '../../../shared/validadores';
import { AuthLayout } from '../auth-layout';

@Component({
  selector: 'app-cadastro',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    AuthLayout,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './cadastro.html',
  styleUrl: '../auth-pagina.scss',
})
export class Cadastro {
  private readonly usuarioService = inject(UsuarioService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);
  private readonly notificacao = inject(NotificacaoService);

  protected readonly dicaSenha = DICA_SENHA;

  protected readonly form = inject(NonNullableFormBuilder).group(
    {
      nome: ['', [Validators.required, Validators.maxLength(150)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      senha: ['', [Validators.required, validadorSenha]],
      confirmacao: ['', Validators.required],
    },
    { validators: senhasIguais('senha', 'confirmacao') },
  );

  protected readonly enviando = signal(false);

  protected cadastrar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { nome, email, senha } = this.form.getRawValue();
    this.enviando.set(true);

    // Cria a conta e já faz o login em seguida
    this.usuarioService
      .cadastrar({ nome: nome.trim(), email, senha })
      .pipe(switchMap(() => this.auth.login({ email, senha })))
      .subscribe({
        next: () => {
          this.notificacao.sucesso(`Bem-vindo(a), ${nome.trim()}!`);
          this.router.navigate(['/']);
        },
        error: () => this.enviando.set(false),
      });
  }
}
