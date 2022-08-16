import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-api-response',
  templateUrl: './view-api-response.component.html',
  styleUrls: ['./view-api-response.component.css']
})
export class ViewApiResponseComponent implements OnInit {

  @Input() apiResponse:any;

  constructor() { }

  ngOnInit(): void {
  }

}
