import axios from 'axios'

var ajax = axios.create({
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded;charset=UTF-8',
    },
    params:{},
    data:{}
});

export default ajax