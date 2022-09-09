import { Component, Input, OnInit } from '@angular/core';
import { IdentityProviders } from '../config/auth-config.module';
import { UserDelegationService } from './user-delegation.service';

@Component({
  selector: 'app-user-delegation',
  templateUrl: './user-delegation.component.html',
  styleUrls: ['./user-delegation.component.css']
})
export class UserDelegationComponent implements OnInit {

  delegationTokenClaims: any = '';

  constructor(private userDelegationService: UserDelegationService) {
    this.userDelegationService.delgationToken$.subscribe(tokenClaims => this.delegationTokenClaims = tokenClaims);
   }


  tryPerformDelegation(consentGrantedMasqueade: boolean){
    this.delegationTokenClaims = '';
    this.userDelegationService.requestTokenExchange(IdentityProviders.KmdAd, IdentityProviders.KmdAd, consentGrantedMasqueade);
  }
   
  ngOnInit(): void {
  }
}