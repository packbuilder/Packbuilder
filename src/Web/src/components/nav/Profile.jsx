import React from "react";
import { Link } from "react-router";
import store from "../../store";

function Profile() {
    
    let state = store();
    let  { addBreadcrumb } = state;

    return <Link to="/Profile">
        <img className="cursor-pointer" onClick={() => addBreadcrumb({text: "Profile", link:"/profile"})} style={{height: "30px", width: "30px", borderRadius: "50%"}} src="./src/mac-and-cheese.jpg" alt="profile icon" />
    </Link>
}

export default Profile