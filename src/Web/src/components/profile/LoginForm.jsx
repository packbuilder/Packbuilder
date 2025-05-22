import React, { useState } from 'react';
import axios from "axios";
import { FaCheck } from 'react-icons/fa';
import store from "../../store";

function LoginForm() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [message, setMessage] = useState('');

  let state = store();
  let { setCurUser } = state;
  
  // const response = await fetch('/sessions', {
  //   method: 'POST',
  //   headers: {
  //     'Content-Type': 'application/json',
  //   },
  //   body: JSON.stringify({ email, password }),
  // });
  

  const handleSubmit = async (event) => {
    event.preventDefault();

    setCurUser();

    try {
      const response = await axios.post("/sessions", {headers: {'Content-Type': 'application/json',}})
      const data = response.data;
      if (response.ok) {
        setMessage(`Welcome, ${data.user.email}`);
      } else {
        setMessage(data.error);
      }
    } catch (error) {
      setMessage('An error occurred. Please try again.');
    }
  };

  return (
    <form onSubmit={handleSubmit} className="">
      <label htmlFor="email">Email</label>
      <input 
        type="email" 
        id="name" 
        className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2" 
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        placeholder="Email"
        required     
      />
      <label htmlFor="password">Password</label>
      <input
        type="password" 
        id="password" 
        className="block shadow rounded-md border border-gray-400 outline-none px-3 py-2 my-2 w-full text-black bg-white focus:border-blue-600 focus:border-2"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        placeholder="Password"
        required
      />
      <button className="cursor-pointer p-3 rounded text-white bg-blue-400 mt-3 self-end"><FaCheck /></button>
      <p>{message}</p>
    </form>
  );
}

export default LoginForm;