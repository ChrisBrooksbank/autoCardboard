import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-pandemic-state.component.html'
})
export class FetchPandemicStateComponent {
  public pandemicState: PandemicState;

  public city = function (city: string) {
    var node = this.pandemicState.cities.filter( mapNode => mapNode.city === city)[0];
    var content = node.city;
    if (node.diseaseCubeCount > 0)
    {
      content += " :" + node.diseaseCubeCount;
    }
  
    return content;
  }

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<string>(baseUrl + 'Play?Game=Pandemic').subscribe(result => {    
        this.pandemicState = JSON.parse(result);
    }, error => console.error(error));
  }
}
