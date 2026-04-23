import { Card } from "react-bootstrap";

function CustomCard({ title, value, icon}) {
    return (
        <Card className='rounded-4 p-2 h-100' style={{ backgroundColor: '#F5EFE7', borderTop: '5px solid #D4A574' }}>
            <Card.Body className='d-flex justify-content-between align-items-center'>
                <div className='text-start'>
                    <Card.Title className='fw-normal' style={{ fontSize: '20px', color: '#6B5E4A' }}>
                        {title}
                    </Card.Title>
                    <Card.Text className='fs-1 fw-bold mb-0'>
                        {value}
                    </Card.Text>
                </div>
                <div className='d-flex justify-content-center align-items-center flex-shrink-0'
                    style={{ backgroundColor: '#EEDFCC', borderRadius: "50%", width: "60px", height: "60px" }}>
                    <i
                        className={icon}
                        style={{ fontSize: "26px", color: "#7C572D" }}></i>
                </div>
            </Card.Body>
        </Card>
    )
}

export default CustomCard;