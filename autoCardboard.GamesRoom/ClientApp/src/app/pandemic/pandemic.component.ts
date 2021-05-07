import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './pandemic.component.html'
})
export class PandemicComponent {
  public pandemicState: PandemicState;
  public play;

  public city = function (city: string) {
    const node = this.pandemicState.cities.filter( mapNode => mapNode.city === city)[0];
    let content = node.city;
    if (node.diseaseCubeCount > 0) {
      content += ' :' + node.diseaseCubeCount;
    }
    return content;
  };

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.play = function() {
      this.pandemicState = null;
      http.get<string>(baseUrl + 'Play?Game=Pandemic').subscribe(result => {
        this.pandemicState = JSON.parse(result);
    }, error => console.error(error));
    };
    this.play();
  }
}
