import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-today-page',
  imports: [MatButtonModule],
  templateUrl: './today-page.html',
  styleUrl: './today-page.css',
})
export class TodayPage {

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
