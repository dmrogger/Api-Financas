import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

/** Componente raiz: só exibe a rota atual */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: '<router-outlet />',
})
export class App {}
