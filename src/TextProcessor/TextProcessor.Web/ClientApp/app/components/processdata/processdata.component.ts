import { Component, Inject } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { NgProgressService } from "ng2-progressbar";

@Component({
    selector: 'processdata',
    templateUrl: './processdata.component.html'
})
export class ProcessdataComponent {
    public searchTextModel: InputTextModel;
    public statisticsResult: StatisticsResponse;
    public forecasts: boolean;
    public showStatistics = false;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string, private pService: NgProgressService, ) {
        this.forecasts = false;
        this.searchTextModel = new InputTextModel();
        this.statisticsResult = new StatisticsResponse();
    }

    public getStatistics() {
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });
        this.pService.start();
        this.http.post(this.baseUrl + 'api/texts/statistics', this.searchTextModel, options).subscribe(result => {
            this.showStatistics = true;
            var respponse = result.json() as StatisticsResponse;
            this.statisticsResult = respponse;
        }, error => console.error(error));
        this.pService.done();
    }
}

class InputTextModel {
    public text: string;
}

class StatisticTypeClass {
    public statisticType: string;
    public count: number;
}

class StatisticsResponse {
    public data: StatisticTypeClass[];
    public statusCode: string;
    public message: string;
}



