import React, { useState } from "react";
import { Footer, Header } from "../Components/Layout";
import { Routes, Route } from "react-router-dom";
import { useEffect } from "react";
import { useDispatch, useSelector } from "react-redux";
import { useGetShoppingCartQuery } from "../Apis/shoppingCartApi";
import { setShoppingCart } from "../storage/Redux/shoppingCartSlice";
import jwt_decode from "jwt-decode";
import { userModel } from "../interface";
import { setLoggedInUser } from "../storage/Redux/userAuthSlice";
import { RootState } from "../storage/Redux/store";
import AuthenticationTestAdmin from "../Pages/AuthenticationTestAdmin";
import Home from "../Pages/Home";
import MenuItemDetails from "../Pages/MenuItemDetails";
import ShoppingCart from "../Pages/ShoppingCart";
import Login from "../Pages/Login";
import Register from "../Pages/Register";
import NotFound from "../Pages/NotFound";
import MenuItemUpsert from "../Pages/Order/AllOrders";
import {MenuItemList} from "../Components/Page/Home";
import OrderDetails from "../Pages/Order/OrderDetails";
import AllOrders from "../Pages/Order/AllOrders";
import MyOrders from "../Pages/Order/MyOrders";
import OrderConfirmed from "../Pages/Order/OrderConfimed";
import Payment from "../Components/Page/Order/Payment";
import AccessDenied from "../Pages/AccessDenied";
import Payments from "../Pages/Payments";
import AuthenticationTest from "../Pages/AuthenticationTest";

function App() {
    const dispatch = useDispatch();
    const [skip, setSkip] = useState(true);
    const userData = useSelector((state: RootState) => state.userAuthStore);
    const { data, isLoading } = useGetShoppingCartQuery(userData.id, {
        skip: skip,
    });

    useEffect(() => {
        const localToken = localStorage.getItem("token");
        if (localToken) {
            const { fullName, id, email, role }: userModel = jwt_decode(localToken);
            dispatch(setLoggedInUser({ fullName, id, email, role }));
        }
    }, []);

    useEffect(() => {
        if (!isLoading && data) {
            console.log(data);
            dispatch(setShoppingCart(data.result?.cartItems));
        }
    }, [data]);

    useEffect(() => {
        if (userData.id) setSkip(false);
    }, [userData]);

    return (
        <div>
            <Header />
            <div className="pb-5">
                <Routes>
                    <Route path="/" element={<Home />}></Route>
                    <Route
                        path="/menuItemDetails/:menuItemId"
                        element={<MenuItemDetails />}
                    ></Route>
                    <Route path="/shoppingCart" element={<ShoppingCart />}></Route>
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />

                    <Route
                        path="/authentication"
                        element={<AuthenticationTest />}
                    ></Route>
                    <Route
                        path="/authorization"
                        element={<AuthenticationTestAdmin />}
                    ></Route>
                    <Route path="/accessDenied" element={<AccessDenied />} />
                    <Route path="/payment" element={<Payments />} />
                    <Route
                        path="order/orderconfirmed/:id"
                        element={<OrderConfirmed />}
                    ></Route>
                    <Route path="/order/myOrders" element={<MyOrders />} />
                    <Route path="/order/orderDetails/:id" element={<OrderDetails />} />
                    <Route path="/order/allOrders" element={<AllOrders />} />
                    <Route path="/menuItem/menuitemlist" element={<MenuItemList />} />
                    <Route
                        path="/menuItem/menuItemUpsert/:id"
                        element={<MenuItemUpsert />}
                    />
                    <Route path="/menuItem/menuItemUpsert" element={<MenuItemUpsert />} />
                    <Route path="*" element={<NotFound />}></Route>
                </Routes>
            </div>
            <Footer />
        </div>
    );
}

export default App;
