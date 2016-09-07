"use strict";
var router_1 = require('@angular/router');
var taskmail_routes_1 = require('./taskmail/taskmail.routes');
var taskmail_component_1 = require('./taskmail/taskmail.component');
var appRoutes = [
    { path: 'Home/AfterLogIn', component: taskmail_component_1.TaskMailComponent }
].concat(taskmail_routes_1.TaskMailRoutes, [
    { path: 'task', component: taskmail_component_1.TaskMailComponent },
]);
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routes.js.map