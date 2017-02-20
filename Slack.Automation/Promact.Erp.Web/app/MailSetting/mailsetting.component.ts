import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MailSettingService } from './mailsetting.service';

@Component({
    template: `
    <router-outlet></router-outlet>
`,
    providers: [MailSettingService]

})
export class MailSettingComponent { }