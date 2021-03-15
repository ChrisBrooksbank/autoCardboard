import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { factoryPolicyTopic, NgxMqttLiteService } from 'ngx-mqtt-lite';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './pandemic.component.html'
})
export class PandemicComponent implements OnInit {
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

 
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, public mqtt: NgxMqttLiteService) {      
   
    this.play = function() {
      this.pandemicState = null;
      http.get<string>(baseUrl + 'Play?Game=Pandemic').subscribe(result => {    
        this.pandemicState = JSON.parse(result);
    }, error => console.error(error));
    };
    this.play();
  }

  // TODO wire up subscription to relevant MQTT topics
  // see https://www.npmjs.com/package/ngx-mqtt-lite
  // also : https://www.npmjs.com/package/ngx-mqtt-lite/v/7.2.1
  // https://github.com/chkr1011/MQTTnet/issues/680
  ngOnInit(): void {
    this.mqtt.registerClient('autocardboard.GamesRoom', null, 
    {
      protocol : "ws", 
      port: 5000, 
      hostname : "localhost"
    }).subscribe(status => { });

    const topic = factoryPolicyTopic([
      { topic: '1/Thought/1', policy: 0, username: '' }
    ]);  
    
    this.mqtt.client('autocardboard.GamesRoom').create(topic).subscribe(result => {
      console.log(result.client.connected);
    }); 
  }
}
