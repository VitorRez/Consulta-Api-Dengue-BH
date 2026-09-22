import { getWeek } from "../api/dengueApi";

export function getIsoWeek(date){
    const d = new Date(Date.UTC(date.getFullYear(), date.getMonth(), date.getDate()));
    const dayNum = d.getUTCDay() || 7;
    d.setUTCDate(d.getUTCDate() + 4 - dayNum);
    const yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
    const weekNo = Math.ceil((((d.getTime() - yearStart.getTime()) / 86400000) - 1) / 7);
    return {ew: weekNo, ey: d.getUTCFullYear()};
}

export async function getLastWeeksWithData(count, maxAttempts = 12){
    const weeks = [];
    const today = new Date();

    for(let i = 1; i <= maxAttempts && weeks.length < count; i++){
        const d = new Date(today);
        d.setDate(d.getDate() - i * 7);

        const {ew, ey} = getIsoWeek(d);
        const data = await getWeek(ew, ey);

        if(data !== null){
            weeks.push(data);
        }else{
            console.log(`Sem dados para ${ey}-${ew}, tentando a anterior.`);
        }
    }

    if(weeks.length < count){
        console.warn(`Apenas ${weeks.length} de ${count} semanas encontradas.`);
    }

    return weeks;
}