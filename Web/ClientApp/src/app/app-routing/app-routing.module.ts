import {NgModule} from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {AuthGuardService} from "../user/auth-guard.service";
import {PageNotFoundComponent} from "../page-not-found/page-not-found.component";
import {HomeComponent} from "../home/home.component";
import {AssignmentsComponent} from "../assignments/assignments.component";
import {InformatieschermComponent} from "../informatiescherm/informatiescherm.component";
import {CategoriesComponent} from "../categories/categories.component";
import {CategoryComponent} from "../category/category.component";
import {ExhibitorsComponent} from "../exhibitors/exhibitors.component";
import {AssignmentDetailComponent} from "../assignment-detail/assignment-detail.component";
import {ExhibitorComponent} from "../exhibitor/exhibitor.component";
import {QuestionsComponent} from "../questions/questions.component";
import {QuestionComponent} from "../questions/question/question.component";
import {RouteMapComponent} from "../route-map/route-map.component";
import {RequestsComponent} from "../user/invitation/pending-requests/requests.component";
import {InviteRequestComponent} from "../user/invitation/send-request/invite-request.component";
import {AskQuestionComponent} from "../ask-question/ask-question.component";

const appRoutes: Routes = [
  {path: 'home', component: HomeComponent},
  {
    path: 'groepen',
    canActivate: [AuthGuardService],
    loadChildren: 'app/all-modules/group/group.module#GroupModule',
    data: {preload: true}
  },
  {path: 'opdrachten', canActivate: [AuthGuardService], component: AssignmentsComponent},
  {path: 'informatiescherm', component: InformatieschermComponent},
  {path: 'categorieen', canActivate: [AuthGuardService], component: CategoriesComponent},
  {path: 'categorie', canActivate: [AuthGuardService], component: CategoryComponent},
  {path: 'exposanten', canActivate: [AuthGuardService], component: ExhibitorsComponent},
  {path: 'assignmentdetail', canActivate: [AuthGuardService], component: AssignmentDetailComponent},
  {path: 'exposant', canActivate: [AuthGuardService], component: ExhibitorComponent},
  {path: 'questions', canActivate: [AuthGuardService], component: QuestionsComponent},
  {path: 'question', canActivate: [AuthGuardService], component: QuestionComponent},
  {path: 'routeMap', canActivate: [AuthGuardService], component: RouteMapComponent},
  {path: 'invite-request', component: InviteRequestComponent},
  {path: 'requests', canActivate: [AuthGuardService], component: RequestsComponent},
  {path: 'ask-question', component: AskQuestionComponent},
  {path: '', redirectTo: 'home', pathMatch: 'full'},
  {path: '**', component: PageNotFoundComponent}
];

@NgModule({
  imports: [
    RouterModule.forRoot(appRoutes)
  ],
  // providers: [SelectivePreloadStrategy],
  declarations: [],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
