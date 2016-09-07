"use strict";
var router_1 = require('@angular/router');
var leaveReport_routes_1 = require('./leaveReport/leaveReport.routes');
var leaveReport_component_1 = require('./leaveReport/leaveReport.component');
var appRoutes = [
    { path: 'Home/AfterLogIn', component: leaveReport_component_1.LeaveReportComponent }
].concat(leaveReport_routes_1.LeaveReportRoutes, [
    { path: 'leave', component: leaveReport_component_1.LeaveReportComponent }
]);
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routes.js.map