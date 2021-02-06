import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-pandemic-state.component.html'
})
export class FetchPandemicStateComponent {
  public pandemicState: PandemicState;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<string>(baseUrl + 'Game').subscribe(result => {    
        this.pandemicState = JSON.parse(result);
    }, error => console.error(error));
  }
}
