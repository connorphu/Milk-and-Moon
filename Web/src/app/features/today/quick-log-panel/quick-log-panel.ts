import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-quick-log-panel',
  imports: [MatButtonModule],
  templateUrl: './quick-log-panel.html',
  styleUrl: './quick-log-panel.css',
})
export class QuickLogPanel{
  public feed() {
    console.log('feed');
  }

  public diaper() {
    console.log('diaper');
  }

  public sleep() {
    console.log('sleep');
  }

  public pump() {
    console.log('pump');
  }
}
