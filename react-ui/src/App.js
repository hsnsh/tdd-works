import axios from "axios";
import React from "react";

import './App.css';

const client = axios.create({
  baseURL: "http://localhost:5000/api/v1/Products"
});

function App() {

  const [allPost, setAllPost] = React.useState(null);
  const [detailPost, setDetailPost] = React.useState(null);

  React.useEffect(() => {
    async function getProducts() {
      const response = await client.get("");
      setAllPost(response.data);
    }
    getProducts();
  }, []);

  if (!allPost) return null;

  return (
    <>
      <div class="container">
        <h1>All Products</h1>
        <ul>
          {allPost.map(d => (
            <li key={d.id}>
              {d.name} 
              <button style={{margin:5}} onClick={async () => {
                const response = await client.get("/" + d.id);
                setDetailPost(response.data);
              }}>Get Detail</button>
            </li>
          ))}
        </ul>
        {
          detailPost != null ?
            <>
              <h1>Details [ {detailPost.id} ]</h1>
              <p>Name: {detailPost.name}</p>
              <p>Price: {detailPost.price}</p>
              <p>Stock: {detailPost.stock}</p>
            </>
            : null
        }
      </div>
    </>
  );
}

export default App;
