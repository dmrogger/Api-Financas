import { BreakpointObserver } from '@angular/cdk/layout';
import { Component, computed, inject, viewChild } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenav, MatSidenavModule } from '@angular/material/sidenav';
import { MatTooltipModule } from '@angular/material/tooltip';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { iniciais } from '../../shared/texto';

interface ItemMenu {
  rota: string;
  icone: string;
  rotulo: string;
}

/** Layout da área logada: menu lateral + conteúdo da rota */
@Component({
  selector: 'app-shell',
  imports: [
    RouterOutlet,
    RouterLink,
    RouterLinkActive,
    MatSidenavModule,
    MatIconModule,
    MatButtonModule,
    MatTooltipModule,
  ],
  templateUrl: './shell.html',
  styleUrl: './shell.scss',
})
export class Shell {
  private readonly auth = inject(AuthService);

  protected readonly usuario = this.auth.usuario;
  protected readonly iniciais = computed(() => iniciais(this.usuario()?.nome));

  /** Em telas pequenas o menu vira "gaveta" que abre por cima do conteúdo */
  protected readonly telaPequena = toSignal(
    inject(BreakpointObserver)
      .observe('(max-width: 900px)')
      .pipe(map((r) => r.matches)),
    { initialValue: false },
  );

  private readonly menu = viewChild.required<MatSidenav>('menu');

  protected readonly itensMenu: ItemMenu[] = [
    { rota: '/', icone: 'space_dashboard', rotulo: 'Resumo' },
    { rota: '/movimentacoes', icone: 'receipt_long', rotulo: 'Movimentações' },
    { rota: '/categorias', icone: 'sell', rotulo: 'Categorias' },
    { rota: '/perfil', icone: 'person', rotulo: 'Meu perfil' },
  ];

  protected aoNavegar(): void {
    if (this.telaPequena()) this.menu().close();
  }

  protected sair(): void {
    this.auth.logout();
  }
}
