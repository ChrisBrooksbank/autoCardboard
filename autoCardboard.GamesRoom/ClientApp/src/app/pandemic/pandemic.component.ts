import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { factoryPolicyTopic, NgxMqttLiteService } from 'ngx-mqtt-lite';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './pandemic.component.html'
})
export class PandemicComponent {
  public pandemicState: PandemicState;
  public play;

  public city = function (city: string) {
    var node = this.pandemicState.cities.filter( mapNode => mapNode.city === city)[0];
    var content = node.city;
    if (node.diseaseCubeCount > 0)
    {
      content += " :" + node.diseaseCubeCount;
    }
  
    return content;
  }

  // TODO wire up subscription to relevant MQTT topics
  // see https://www.npmjs.com/package/ngx-mqtt-lite
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public mqtt: NgxMqttLiteService) {      
    const topic = factoryPolicyTopic([
      { topic: '1/Thought/1', policy: 0, username: '' }
    ]);  

    this.play = function() {
      this.pandemicState = null;
      http.get<string>(baseUrl + 'Play?Game=Pandemic').subscribe(result => {    
        this.pandemicState = JSON.parse(result);
    }, error => console.error(error));
    };
    this.play();
  }
}
