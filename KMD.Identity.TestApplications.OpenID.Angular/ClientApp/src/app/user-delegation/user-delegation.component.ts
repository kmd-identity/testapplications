import { Component, Input, OnInit } from '@angular/core';
import { UserDelegationService } from './user-delegation.service';

@Component({
  selector: 'app-user-delegation',
  templateUrl: './user-delegation.component.html',
  styleUrls: ['./user-delegation.component.css']
})
export class UserDelegationComponent implements OnInit {

  @Input() accessToken: string = '';
  delegationTokenClaims: any = '';

  constructor(private userDelegationService: UserDelegationService) {
    this.userDelegationService.delgationToken$.subscribe(tokenClaims => this.delegationTokenClaims = tokenClaims);
   }

  ngOnInit(): void {
    this.userDelegationService.requestDelegationToken();
  }
}