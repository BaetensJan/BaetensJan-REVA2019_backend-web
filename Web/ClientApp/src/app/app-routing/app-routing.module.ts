import {NgModule} from '@angular/core';
import {RouterModule, Routes} from "@angular/router";
import {AuthGuardService} from "../user/auth-guard.service";
import {PageNotFoundComponent} from "../page-not-found/page-not-found.component";
import {HomeComponent} from "../home/home.component";
import {InformatieschermComponent} from "../informatiescherm/informatiescherm.component";
import {CategoriesComponent} from "../categories/categories.component";
import {CategoryComponent} from "../category/category.component";
import {ExhibitorsComponent} from "../exhibitors/exhibitors.component";
import {ExhibitorComponent} from "../exhibitor/exhibitor.component";
import {QuestionsComponent} from "../questions/questions.component";
import {QuestionComponent} from "../questions/question/question.component";
import {RouteMapComponent} from "../route-map/route-map.component";
import {AskQuestionComponent} from "../ask-question/ask-question.component";
import {RequestsComponent} from "../invitation/pending-requests/requests.component";
import {AssignmentsComponent} from "../assignment/assignments-overview/assignments.component";
import {AssignmentDetailComponent} from "../assignment/assignment-detail/assignment-detail.component";
import {UploadCsvComponent} from "../upload-csv/upload-csv.component";
import {ForgotPasswordConfirmationComponent} from "../user/forgot-password-confirmation/forgot-password-confirmation.component";
import {ForgotPasswordComponent} from "../user/forgot-password/forgot-password.component";
import {ResetPasswordComponent} from "../user/reset-password/reset-password.component";
import {ChangePasswordComponent} from "../user/change-password/change-password.component";
import {InviteRequestComponent} from "../invitation/send-or-update-request/invite-request.component";
import {AcceptRequestComponent} from "../invitation/accept-request/accept-request.component";
import {EnableTourComponent} from "../enableTour/enableTour.component";

const appRoutes: Routes = [
  {path: 'home', component: HomeComponent},

  /**
   * Teacher pages
   */
  {
    path: 'group',
    canActivate: [AuthGuardService],
    loadChildren: 'app/all-modules/group/group.module#GroupModule',
    data: {preload: true}
  },
  {path: 'opdrachten', canActivate: [AuthGuardService], component: AssignmentsComponent},
  {path: 'assignmentdetail', canActivate: [AuthGuardService], component: AssignmentDetailComponent},
  {path: 'informatiescherm', component: InformatieschermComponent},

  /**
   * Admin pages
   */
  {path: 'categorieen', canActivate: [AuthGuardService], component: CategoriesComponent},
  {path: 'categorie', canActivate: [AuthGuardService], component: CategoryComponent},
  {path: 'exposanten', canActivate: [AuthGuardService], component: ExhibitorsComponent},
  {path: 'exposant', canActivate: [AuthGuardService], component: ExhibitorComponent},
  {path: 'upload-csv', canActivate: [AuthGuardService], component: UploadCsvComponent},
  {path: 'vragen', canActivate: [AuthGuardService], component: QuestionsComponent},
  // detail page of a specific Assignment Question
  {path: 'vraag', canActivate: [AuthGuardService], component: QuestionComponent},
  {path: 'beursplan', canActivate: [AuthGuardService], component: RouteMapComponent},

  /**
   * Request pages
   */
  {path: 'invite-request', canActivate: [AuthGuardService], component: InviteRequestComponent},
  {path: 'accept-request', canActivate: [AuthGuardService], component: AcceptRequestComponent},
  {path: 'requests', canActivate: [AuthGuardService], component: RequestsComponent},

  /**
   * Common pages
   */
  {path: 'ask-question', component: AskQuestionComponent},

  /**
   * Password pages
   */
  {path: 'forgot-password', canActivate: [AuthGuardService], component: ForgotPasswordComponent},
  {path: 'wachtwoord-veranderen', canActivate: [AuthGuardService], component: ChangePasswordComponent},
  {
    path: 'wachtwoord-vergeten-confirmation', canActivate: [AuthGuardService],
    component: ForgotPasswordConfirmationComponent
  },
  {path: 'reset-wachtwoord', canActivate: [AuthGuardService], component: ResetPasswordComponent},
  {path: 'enable-tour', canActivate: [AuthGuardService], component: EnableTourComponent},

  /**
   * Fallback
   */
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
