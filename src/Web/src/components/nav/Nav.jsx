import React from "react";
import BreadCrumb from "./BreadCrumb";
import Profile from "./Profile";

function Nav() {
    return (
    <nav 
    className="fixed top-0 left-0 flex flex-row justify-between items-center px-5 h-[9vh] w-full"
    style={{background: "rgba(255, 255, 255, 0.2)", boxShadow:"0 4px 30px rgba(0, 0, 0, 0.1)",backdropFilter: "blur(12.1px)", WebkitBackdropFilter: "blur(5px)", border:" 1px solid rgba(255, 255, 255, 0.3)",}}>
        <BreadCrumb />
        <Profile />
    </nav>)
}

export default Nav