import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-claims',
  templateUrl: './user-claims.component.html',
  styleUrls: ['./user-claims.component.css']
})
export class UserClaimsComponent implements OnInit {

  @Input() userClaims: any;

  constructor() { }

  ngOnInit(): void {
  }
}
