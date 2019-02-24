import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {UpdateGroupComponent} from '../../groups/update-group/update-group.component';
import {GroupsComponent} from '../../groups/groups.component';
import {GroupsDataService} from '../../groups/groups-data.service';
import {AuthGuardService} from "../../user/auth-guard.service";
import {HttpClientModule} from "@angular/common/http";
import {RouterModule} from "@angular/router";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {httpInterceptorProviders} from '../../http-interceptors';
import {AccordionModule, PaginationModule, TypeaheadModule} from "ngx-bootstrap";

const routes = [
  {path: 'updateGroup', canActivate: [AuthGuardService], component: UpdateGroupComponent},
  {path: 'groups', canActivate: [AuthGuardService], component: GroupsComponent},
  // {
  //   path: ':id',
  //   component: UpdateGroupComponent,
  //   resolve: { group: GroupResolver }
  // }
];

@NgModule({
  declarations: [
    GroupsComponent,
    UpdateGroupComponent
  ],
  imports: [
    AccordionModule.forRoot(),
    PaginationModule,
    TypeaheadModule.forRoot(),
    FormsModule,
    HttpClientModule,
    CommonModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ],
  providers: [
    httpInterceptorProviders,
    GroupsDataService
  ],
})
export class GroupModule {
}
