import React from "react";
import { MdOutlineSaveAlt } from "react-icons/md";
import { HiOutlineCog } from "react-icons/hi";
import { IoIosAdd } from "react-icons/io";

function modpackView(props) {
    return <section className="min-w-[300px] mt-[12vh] mb-3">
        <div className="flex flex-col justify-center items-center mb-4">
            <img src="./src/modpack.gif" alt="Modpack logo" className="bg-black aspect-square w-28 h-28 md:w-40 md:h-40" />
            <h1 className="text-3xl font-bold">The woah</h1>
            <h2 className="opacity-60 text-lg">Modpack link (clipboard emoji here)</h2>
        </div>

        <div className="flex flex-col justify-center items-center">
            <h1 className="text-3xl font-bold">Mods</h1>
            <form action="something about searching idk" className="flex flex-row justify-between items-center w-full">
                <input className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" type="text" placeholder="Search Mods..."/>
                <button className="ml-2 bg-blue-400 rounded font-bold text-4xl hover:bg-blue-300 cursor-pointer"><IoIosAdd /></button>
            </form>
        </div>

        {/* This should display the text if there are no mods in the modpack, otherwise mods will be displayed in this div */}
        <div className="flex flex-col justify-center items-center min-w-[300px] min-h-[400px] border max-w-full border-black dark:border-gray-400 bg-gray-700 flex flex-col h-96 w-96 overflow-y-auto overflow-x-clip w-full">
            <h1 className="">It's looking empty in here...</h1>
        </div>

        <div className="flex flex-row justify-end items-center py-4">
            <button className="bg-black rounded font-bold text-4xl hover:bg-gray-800 cursor-pointer"><HiOutlineCog /></button>
            <button className="ml-4 bg-blue-400 rounded font-bold text-4xl hover:bg-blue-300 cursor-pointer"><MdOutlineSaveAlt /></button>
        </div>
    </section>
}

export default modpackView;