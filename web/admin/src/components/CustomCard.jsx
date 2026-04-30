import { Card } from "react-bootstrap";
import { useContext } from "react";
import { ThemeContext } from "./Theme";

function CustomCard({ title, value, icon }) {
    const { dark, toggleTheme } = useContext(ThemeContext);

    return (
        <Card className='rounded-4 p-2 h-100 transition' style={{ backgroundColor: dark ? '#2A2A2A' : '#F5EFE7', borderTop: '5px solid #D4A574' }}>
            <Card.Body className='d-flex justify-content-between align-items-center'>
                <div className='text-start'>
                    <Card.Title className='fw-normal transition' style={{ fontSize: '20px', color: dark ? '#A0A0A0' : '#6B5E4A' }}>
                        {title}
                    </Card.Title>
                    <Card.Text className='fs-1 fw-bold mb-0 transition' style={{ color: dark ? '#F5F5F5' : '#000' }} >
                        {value}
                    </Card.Text>
                </div>
                <div className='d-flex justify-content-center align-items-center flex-shrink-0 transition'
                    style={{ backgroundColor: dark ? 'rgba(212, 165, 116, 0.15)' : '#EEDFCC', borderRadius: "50%", width: "60px", height: "60px" }}>
                    <i
                        className={`${icon} transition`}
                        style={{ fontSize: "26px", color: dark ? "#D4A574" : "#7C572D" }}></i>
                </div>
            </Card.Body>
        </Card>
    )
}

export default CustomCard;