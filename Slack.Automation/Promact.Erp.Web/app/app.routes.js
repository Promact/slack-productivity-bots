"use strict";
var router_1 = require('@angular/router');
var leaveReport_routes_1 = require('./leaveReport/leaveReport.routes');
var leaveReport_component_1 = require('./leaveReport/leaveReport.component');
var scrumReport_component_1 = require('./ScrumReport/scrumReport.component');
var scrumReport_routes_1 = require('./ScrumReport/scrumReport.routes');
var appRoutes = [
    { path: 'Home/AfterLogIn', component: leaveReport_component_1.LeaveReportComponent }
].concat(leaveReport_routes_1.LeaveReportRoutes, [
    { path: 'leave', component: leaveReport_component_1.LeaveReportComponent }
], scrumReport_routes_1.ScrumReportRoutes, [
    { path: 'scrum', component: scrumReport_component_1.ScrumReportComponent }
]);
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routes.js.map