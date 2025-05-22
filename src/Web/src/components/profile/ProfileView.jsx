import React from "react";
import { Link } from "react-router";
import store from "../../store";

function ProfileView(props) {
    let state = store();
    let { addBreadcrumb } = state;

    return <section className="flex flex-col justify-center items-center">
        <div className="flex flex-col justify-center items-center mb-2">
            <img className="rounded-full w-24" src="./src/mac-and-cheese.jpg" alt="profile icon" />
            <h1 className="font-bold text-2xl m-2">Goob Dev</h1>
        </div>
        <div className="flex flex-row justify-center items-center">
            <Link to="/edit_profile" onClick={() => addBreadcrumb({text:"Edit profile", link:"/edit_profile"})} className="cursor-pointer text-orange-300 border border-orange-300 px-2 py-1 rounded hover:border-orange-400 hover:text-orange-400 m-2">Edit profile</Link>
            <button className="cursor-pointer text-red-300 border border-red-300 px-2 py-1 rounded hover:border-red-400 hover:text-red-400 m-2">Logout</button>
        </div>
    </section>
}

export default ProfileView