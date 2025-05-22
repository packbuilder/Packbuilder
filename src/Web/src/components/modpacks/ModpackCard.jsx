import React from "react";
import { Link } from "react-router";
import store from "../../store";

function ModpackCard(props) {
    const modpack = {name: props.title, image: props.avatar_url, modCount: props.mod_count};

    const state = store();
    const { setCurModpack, addBreadcrumb } = state;

    return <Link 
    onClick={() => {
        setCurModpack(modpack);
        addBreadcrumb({text:"Modpack view", link: "/modpack_view"});
    }}
    to="/modpack_view"
    id="modpack-home-page" 
    className="duration-100 cursor-pointer focus:shadow focus:scale-110 hover:shadow hover:scale-110 p-2 border dark:border-white flex flex-col gap-2 items-center w-44 rounded" 
    style={{
        background:"rgba(255, 255, 255, 0.2)",
        borderRadius: "16px",
        boxShadow: "0 4px 30px rgba(0, 0, 0, 0.1)",
        backdropFilter: "blur(12.1px)",
        WebkitBackdropFilter: "blur(5px)",
        border:" 1px solid rgba(255, 255, 255, 0.3)"
    }}>
    
        <img src={props.avatar_url} alt={`Logo for ${props.title}`} />
        <h1>{props.title}</h1>
        <h3>{props.mod_count}</h3>
    </Link>
}

export default ModpackCard;