# REVA - WEB Application & Backend

__The goal of the application is:__
* Providing a _distribution system_ in order to avoid overloaded gatherings of visiting school groups at specific, attractive exhibitors at the Reva Exhibition Ghent
* Providing _assignments_, related to the, by a group chosen, exhibitor with questions and possibility of taking pictures and adding notes. This will avoid the frequently asked questions to the exhibitors by groups of students
* Integrated, dynamic _routeplan_ that will guide the groups of student in a well-thought-out _tour_ of visiting the exhibition

__The Web Application will supply for:__
* Mobile (android) _Application Guide_ and option to download as PDF or send via mail
* Overview of a teacher's/school's groups (and corresponding students) with possibility to _create groups and add students_ to groups.
* Administration
   * Creating and editing of exhibitors and their corresponding location on a map,
          with possibility to add or edit corresponding questions.
   * Creating and editing of categories

__Technicalities:__
* Angular 7 front end and ASP.net (with Entitiy Framework) back end.
* Layout: Bootstrap 3 and Material (CSS) and ngx-bootstrap (Javascript components)
* Angular vertical timeline, as to create the Mobile Application Guide (https://www.npmjs.com/package/angular-vertical-timeline)

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 1.4.9.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The app will automatically reload if you change any of the source files.

Running this project in your IDE (e.g. IntelliJ Rider or Visual Studio) will give you the possibility of opening the web application while simultaneously running the backend (including the controllers). In your browser you could then be using `http://localhost:5000` to access the web application.
While in production we use an sqlite database in our /REVA folder, which will later be replaced by using Microsoft Azure SQL Database.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory. Use the `-prod` flag for a production build.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI README](https://github.com/angular/angular-cli/blob/master/README.md).
