import { Component, computed, inject, OnInit, signal } from '@angular/core';
import {
  AbstractControl,
  NonNullableFormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { filter, switchMap } from 'rxjs';
import { UsuarioService } from '../../core/api/usuario.service';
import { AuthService } from '../../core/auth/auth.service';
import { Usuario } from '../../core/models/api.models';
import { NotificacaoService } from '../../core/notificacao.service';
import { confirmar } from '../../shared/confirmacao-dialog';
import { iniciais } from '../../shared/texto';
import { DICA_SENHA, senhasIguais, validadorSenha } from '../../shared/validadores';

@Component({
  selector: 'app-perfil',
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './perfil.html',
  styleUrl: './perfil.scss',
})
export class Perfil implements OnInit {
  private readonly usuarioService = inject(UsuarioService);
  private readonly auth = inject(AuthService);
  private readonly notificacao = inject(NotificacaoService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(NonNullableFormBuilder);

  protected readonly dicaSenha = DICA_SENHA;
  protected readonly usuario = signal<Usuario | null>(null);
  protected readonly iniciais = computed(() => iniciais(this.usuario()?.nome));
  protected readonly salvando = signal(false);
  protected readonly excluindo = signal(false);

  protected readonly formEdicao = this.fb.group(
    {
      nome: ['', [Validators.required, Validators.maxLength(150)]],
      senhaNova: ['', validadorSenha],
      confirmacao: [''],
      senhaAtual: ['', Validators.required],
    },
    { validators: [senhasIguais('senhaNova', 'confirmacao'), this.algoMudou.bind(this)] },
  );

  protected readonly formExclusao = this.fb.group({
    senha: ['', Validators.required],
  });

  ngOnInit(): void {
    this.usuarioService.consultar().subscribe((u) => {
      this.usuario.set(u);
      this.formEdicao.controls.nome.setValue(u.nome);
    });
  }

  protected salvar(): void {
    if (this.formEdicao.invalid) {
      this.formEdicao.markAllAsTouched();
      return;
    }

    const { nome, senhaNova, senhaAtual } = this.formEdicao.getRawValue();
    const nomeAlterado = nome.trim() !== this.usuario()?.nome;

    this.salvando.set(true);
    this.usuarioService
      .editar({
        senhaAtual,
        nome: nomeAlterado ? nome.trim() : null,
        senhaNova: senhaNova || null,
      })
      .subscribe({
        next: () => {
          if (nomeAlterado) {
            this.auth.atualizarNome(nome.trim());
            this.usuario.update((u) => (u ? { ...u, nome: nome.trim() } : u));
          }
          this.formEdicao.reset({ nome: nome.trim(), senhaNova: '', confirmacao: '', senhaAtual: '' });
          this.salvando.set(false);
          this.notificacao.sucesso('Perfil atualizado.');
        },
        error: () => this.salvando.set(false),
      });
  }

  protected excluirConta(): void {
    if (this.formExclusao.invalid) {
      this.formExclusao.markAllAsTouched();
      return;
    }

    confirmar(this.dialog, {
      titulo: 'Excluir conta',
      mensagem:
        'Sua conta, categorias e movimentações serão apagadas definitivamente. Deseja continuar?',
      textoConfirmar: 'Excluir minha conta',
    })
      .pipe(
        filter(Boolean),
        switchMap(() => {
          this.excluindo.set(true);
          return this.usuarioService.excluir(this.formExclusao.getRawValue().senha);
        }),
      )
      .subscribe({
        next: () => {
          this.notificacao.sucesso('Conta excluída.');
          this.auth.logout();
        },
        error: () => this.excluindo.set(false),
      });
  }

  /** A API exige ao menos uma alteração: nome diferente ou senha nova */
  private algoMudou(grupo: AbstractControl): ValidationErrors | null {
    const nome = (grupo.get('nome')?.value ?? '').trim();
    const senhaNova = grupo.get('senhaNova')?.value;
    const nomeAtual = this.usuario?.()?.nome;
    return nome === nomeAtual && !senhaNova ? { semAlteracao: true } : null;
  }
}
