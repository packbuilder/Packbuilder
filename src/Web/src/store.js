import { create } from "zustand";
import Cookies from "js-cookie";

const store = create((set, get) => ({
    curUser: Cookies.get("_packbuilder_session"),
    curModpack: {},
    breadCrumbs: [{text: "Home", link: "/"}],
    setCurUser: () => {
        Cookies.set("_packbuilder_session", "some session id");
        set({curUser: Cookies.get("_packbuilder_session")});
    },
    setCurModpack: (modpack) => {
        set({curModpack: modpack});
    },
    addBreadcrumb: (newBreadCrumb) => {
        let breadCrumbs = get().breadCrumbs;
        let hasVisitedPage = false;

        breadCrumbs.forEach(breadCrumb => {
            if(breadCrumb.link === newBreadCrumb.link) {
                hasVisitedPage = true;
            }
        });

        if(!hasVisitedPage) breadCrumbs.push(newBreadCrumb);

        set({breadCrumbs: breadCrumbs});
    },
    updateBreadCrumbs: (breadCrumb) => {
        let breadCrumbs = get().breadCrumbs;

        while(breadCrumbs[breadCrumbs.length - 1].link !== breadCrumb.link) {
            breadCrumbs.pop();

            if(breadCrumbs.length === 1) {
                break;
            }
        }

        set({breadCrumbs: breadCrumbs});
    },
}));

export default store;