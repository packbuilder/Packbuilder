import React from "react";
import { FaCheck } from "react-icons/fa";
import { MdDelete } from "react-icons/md";

function EditProfile(params) {
    return <section className="min-w-[300px] my-[13vh]">
        <form action="" autoComplete="off" className="flex flex-col justify-center items-start">
            <img className="rounded-full w-40 self-center" src="./src/mac-and-cheese.jpg" alt="profile icon"/>

            <div className="self-center flex flex-col justify-center items-center mt-4">
                <label htmlFor="avatar_label">Avatar</label>
                <label id="avatar_label" htmlFor="avatar" className="cursor-pointer block shadow border border-gray-400 outline-none px-2 py-1 my-2 text-black bg-white text-center w-fit">Choose File</label>
                <input type="file" id="avatar" style={{display: "none"}} />
            </div>

            <label htmlFor="name">Name</label>
            <input type="text" id="name" className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" />

            <label htmlFor="email">Email</label>
            <input type="email" id="email" className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" />

            <details className="mt-3 w-full">
                <summary className="mb-3 cursor-pointer">Change Password</summary>
                    <label htmlFor="password">New Password</label>
                    <input type="password" id="password" className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" />

                    <label htmlFor="password_confirmation">Confirm Password</label>
                    <input type="password_confirmation" id="password_confirmation" className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" />
            </details>

            <div className="self-end">
                <button className="cursor-pointer p-3 rounded text-white bg-blue-400 mt-3 self-end mx-2"><FaCheck /></button>
                <button className="cursor-pointer p-3 rounded text-white bg-red-400 mt-3 self-end ml-2 "><MdDelete /></button>
            </div>
        </form>
    </section>
}

export default EditProfile;