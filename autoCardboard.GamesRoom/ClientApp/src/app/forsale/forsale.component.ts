import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './forsale.component.html'
})
export class ForSaleComponent {
  public forsaleState: ForSaleState;
  public play;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.play = function() {
      this.pandemicState = null;
      http.get<string>(baseUrl + 'Play?Game=ForSale').subscribe(result => {
        this.forsaleState = JSON.parse(result);
    }, error => console.error(error));
    };
    this.play();
  }
}
