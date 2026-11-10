import { Routes } from '@angular/router';
import { authGuard, visitanteGuard } from './core/auth/auth.guards';

// Cada tela é carregada sob demanda (lazy loading) com loadComponent
export const routes: Routes = [
  {
    path: 'login',
    canActivate: [visitanteGuard],
    title: 'Entrar · Minhas Finanças',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },
  {
    path: 'cadastro',
    canActivate: [visitanteGuard],
    title: 'Criar conta · Minhas Finanças',
    loadComponent: () => import('./features/auth/cadastro/cadastro').then((m) => m.Cadastro),
  },
  {
    // Área logada: o Shell tem a barra superior e o menu; as telas aparecem dentro dele
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./layout/shell/shell').then((m) => m.Shell),
    children: [
      {
        path: '',
        title: 'Resumo · Minhas Finanças',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'movimentacoes',
        title: 'Movimentações · Minhas Finanças',
        loadComponent: () =>
          import('./features/movimentacoes/movimentacoes').then((m) => m.Movimentacoes),
      },
      {
        path: 'categorias',
        title: 'Categorias · Minhas Finanças',
        loadComponent: () => import('./features/categorias/categorias').then((m) => m.Categorias),
      },
      {
        path: 'perfil',
        title: 'Perfil · Minhas Finanças',
        loadComponent: () => import('./features/perfil/perfil').then((m) => m.Perfil),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
