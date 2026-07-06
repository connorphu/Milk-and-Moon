import { Component, signal } from '@angular/core';
import { MatSlideToggle } from '@angular/material/slide-toggle';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatSlideToggle],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('Milk and Moon');
}
