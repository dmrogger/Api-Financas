import { AbstractControl, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

/** Mesma regra da API: 8 a 64 caracteres, 1 letra maiúscula e 1 caractere especial */
export const REGEX_SENHA = /^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{8,64}$/;

export const validadorSenha = Validators.pattern(REGEX_SENHA);

export const DICA_SENHA = 'Mínimo de 8 caracteres, com 1 letra maiúscula e 1 caractere especial.';

/** Validador de grupo: o campo "confirmacao" precisa ser igual ao campo "senha" */
export function senhasIguais(campoSenha: string, campoConfirmacao: string): ValidatorFn {
  return (grupo: AbstractControl): ValidationErrors | null => {
    const senha = grupo.get(campoSenha)?.value;
    const confirmacao = grupo.get(campoConfirmacao)?.value;
    return senha && confirmacao && senha !== confirmacao ? { senhasDiferentes: true } : null;
  };
}
