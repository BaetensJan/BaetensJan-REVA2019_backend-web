import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {UserModule} from "./user/user.module";
import {AssignmentsComponent} from "./assignments/assignments.component";
import {InformatieschermComponent} from './informatiescherm/informatiescherm.component';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {VerticalTimelineModule} from 'angular-vertical-timeline';
import {
  ModalModule,
  PaginationModule,
  TypeaheadModule,
  ButtonsModule,
  AlertModule, BsDropdownModule
} from "ngx-bootstrap";
import {ExhibitorComponent} from './exhibitor/exhibitor.component';
import {ExhibitorsComponent} from './exhibitors/exhibitors.component';
import {ExhibitorsDataService} from "./exhibitors/exhibitors-data.service";
import {ExhibitorShareService} from "./exhibitor/exhibitor-share.service";
import {AssignmentDataService} from "./assignments/assignment-data.service";
import {CategoriesDataService} from "./categories/categories-data.service";
import {CategoryShareService} from "./category/category-share.service";
import {CategoriesComponent} from "./categories/categories.component";
import {CategoryComponent} from "./category/category.component";
import {AssignmentDetailComponent } from './assignment-detail/assignment-detail.component';
import {AppShareService} from "./AppShareService";
import { QuestionsComponent } from './questions/questions.component';
import { QuestionComponent } from './questions/question/question.component';
import {QuestionSharedService} from "./questions/question-shared.service";
import {QuestionDataService} from "./questions/question-data.service";
import {FileUploadModule} from "ng2-file-upload";
import { RouteMapComponent } from './route-map/route-map.component';
import {SchoolDataService} from "./schools/school-data.service";
import {ImageDataService} from "./image-data-service/image-data.service";
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import {AppRoutingModule} from "./app-routing/app-routing.module";
import { AskQuestionComponent } from './ask-question/ask-question.component';
import { UploadCsvComponent } from './upload-csv/upload-csv.component';

@NgModule({
  declarations: [
    AppComponent,
    PageNotFoundComponent, HomeComponent, AssignmentsComponent, InformatieschermComponent,
    CategoriesComponent, CategoryComponent, ExhibitorComponent, ExhibitorsComponent, AssignmentDetailComponent,
    QuestionComponent, QuestionsComponent, RouteMapComponent, NavMenuComponent, AskQuestionComponent, UploadCsvComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    UserModule,
    ModalModule.forRoot(),
    BrowserAnimationsModule,
    VerticalTimelineModule,
    PaginationModule.forRoot(),
    FormsModule,
    ReactiveFormsModule,
    TypeaheadModule.forRoot(),
    ButtonsModule.forRoot(),
    AlertModule.forRoot(),
    FileUploadModule,
    BsDropdownModule.forRoot(),
    AppRoutingModule
  ],
  providers: [
    CategoriesDataService,
    CategoryShareService,
    ExhibitorsDataService,
    ExhibitorShareService,
    AssignmentDataService,
    AppShareService,
    QuestionDataService,
    QuestionSharedService,
    SchoolDataService,
    ImageDataService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
