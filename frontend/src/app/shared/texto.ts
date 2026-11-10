/** "Ana Maria Souza" → "AM" */
export function iniciais(nome: string | null | undefined): string {
  return (nome ?? '')
    .split(' ')
    .filter(Boolean)
    .slice(0, 2)
    .map((parte) => parte[0].toUpperCase())
    .join('');
}

/** "Ana Maria Souza" → "Ana" */
export function primeiroNome(nome: string | null | undefined): string {
  return (nome ?? '').trim().split(' ')[0] ?? '';
}
