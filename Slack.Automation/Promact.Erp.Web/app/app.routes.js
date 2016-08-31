"use strict";
var router_1 = require('@angular/router');
var leaveReport_List_component_1 = require('./leaveReport/leaveReport-List/leaveReport-List.component');
var leaveReport_Details_component_1 = require('./leaveReport/leaveReport-Details/leaveReport-Details.component');
var appRoutes = [
    {
        path: 'report',
        component: leaveReport_List_component_1.LeaveReportListComponent
    },
    {
        path: 'detail/:id',
        component: leaveReport_Details_component_1.LeaveReportDetailsComponent
    },
    {
        path: 'Home/AfterLogIn',
        redirectTo: '/report',
        pathMatch: 'full'
    }
];
exports.routing = router_1.RouterModule.forRoot(appRoutes);
//# sourceMappingURL=app.routes.js.map