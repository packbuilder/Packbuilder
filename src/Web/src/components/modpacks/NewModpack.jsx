import React from "react";
import { FaCheck } from "react-icons/fa";

function NewModpack(props) {
    return <section className="min-w-[300px]">
        <form autoComplete="off" action="some function that handles it like the resturaunt app or something" className="flex flex-col justify-center items-start">
            <label htmlFor="name">Name</label>
            <input type="text" id="name" className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" />
            <div>
                <label htmlFor="avatar_label">Avatar</label>
                <label id="avatar_label" htmlFor="avatar" className="block shadow border border-gray-400 outline-none px-2 py-1 my-2 text-black bg-white text-center w-fit">Choose File</label>
                <input type="file" id="avatar" style={{display: "none"}} />
            </div>
            <label htmlFor="game">Game</label>
            <select name="game" id="game" className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2">
                <option value="minecraft">minecraft</option>
                <option value="lethal_company">lethal company</option>
            </select>

            <details className="mt-3">
                <summary>Import your modpack</summary>
                    <label id="import_label" htmlFor="import" className="block shadow border border-gray-400 outline-none px-2 py-1 my-2 text-black bg-white text-center w-fit">Choose File</label>
                    <input type="file" id="import" style={{display: "none"}} />
            </details>

            <button className="cursor-pointer p-3 rounded text-white bg-blue-400 mt-3 self-end"><FaCheck /></button>
        </form>
    </section>
}

export default NewModpack;