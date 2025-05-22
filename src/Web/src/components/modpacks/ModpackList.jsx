import React, { useState, useEffect } from "react";
import { IoIosAdd } from "react-icons/io";
import { Link } from "react-router";
import axios from "axios";
import store from "../../store"
import ModpackCard from "./ModpackCard";

function ModpackList() {

    const url =  "TBD";

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [modpacks, setModpacks] = useState(null);

    useEffect(() => {
        axios.get(url)
            .then(function (response) {
                setModpacks(response.data);
                setLoading(false);
            })
            .catch(function (error) {
                setError(error.status);
                setLoading(false);
            }).finally(function () {
                console.log("Request done.");
            });
      }, []);

    const state = store();
    const { addBreadcrumb } = state;

    if(loading) {
        return <h1>Loading modpacks...</h1>
    }

    if(error) {
        return <h1>{`Error fetching modpacks: ${error.status}`}</h1>
    }

    return <section className="flex flex-col justify-between items-center w-full mx-auto h-full">
        <div className="flex flex-row justify-around items-center mb-10">
            <h1 className="text-3xl font-bold p-2">Modpacks</h1>
            <Link to="/create_modpack" onClick={() => addBreadcrumb({text:"Create modpack", link: "/create_modpack"})} className="bg-blue-400 rounded font-bold text-4xl hover:bg-blue-300 cursor-pointer"><IoIosAdd /></Link>
        </div>
        <div id="modpacks" className="flex flex-row flex-wrap gap-4 min-w-full justify-center items-center">
            <ModpackCard key={1} avatar_url={"./src/modpack.gif"} title={"Woah modpack"} mod_count={"No mods"}/>
            {/* {modpacks.map((modpack, index) => {
                <ModpackCard key={index} {...modpack} />
            })} */}
        </div>
    </section>
}

export default ModpackList