import React from "react";
import { NavLink } from "react-router";
import store from "../../store";

function BreadCrumb() {

    const state = store();
    const { breadCrumbs, updateBreadCrumbs } = state;

    return <div className="flex flex-row justify-around items-center">
        {breadCrumbs.map((breadCrumb, index) => {
            return <NavLink 
            onClick={() => {updateBreadCrumbs(breadCrumb);}}
            key={index} 
            to={breadCrumb.link}
            className={({ isActive }) => {
                const textColor = isActive ? "font-bold" : "";
                let styling = "flex flex-row justify-around items-center "
                
                if(breadCrumb.link !== "/") {
                    styling += "max-md:hidden";
                }

                return `${styling} ${textColor}`;
            }}>
                <button className="cursor-pointer mx-4">{breadCrumb.text}
                </button>
                {breadCrumbs.length - 1 === index ? null : <p className="max-md:hidden">{">"}</p>}
            </NavLink> 
        })}
        
    </div>
}

export default BreadCrumb