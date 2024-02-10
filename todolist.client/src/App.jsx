import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Card, CardHeader, CardBody, Button } from 'reactstrap';
import TablaTasks from './componentes/TableTasks';
import ModalTasks from './componentes/ModalTasks';



const App = () => {
    const [tasks, setTasks] = useState([]);
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(true);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const baseUrl = 'https://localhost:7059/api/Tasks';

    const showTasks = async () => {
        try {
            const response = await fetch(`${baseUrl}/Lista`);

            if (!response.ok) {
                throw new Error(`Error: ${response.status}`);
            }

            const data = await response.json();

            const sortedTasks = data.sort((a, b) => new Date(a.Deadline) - new Date(b.Deadline));

            setTasks(sortedTasks);
            setError(null);
        } catch (error) {
            setError(`Error fetching tasks: ${error.message}`);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        showTasks();
    }, [isModalOpen]); 

    const toggleModal = () => {
        setIsModalOpen(!isModalOpen);
    };

    const handlePageUpdate = () => {
        showTasks(); 
    };

    return (
        <Container fluid className="mt-5">
            <Row className="justify-content-center">
                <Col md="8">
                    <Card>
                        <CardHeader>
                            <h5 className="display-4">Tasks</h5>
                        </CardHeader>
                        <CardBody>
                            <div className="d-flex mb-2">
                                <Button size="lg" color="success" className="mr-2" onClick={toggleModal}>
                                    New Task
                                </Button>
                                {/*<Button size="lg" color="warning">*/}
                                {/*    New Category*/}
                                {/*</Button>*/}
                            </div>
                            <hr className="my-4" />
                            {loading ? (
                                <p className="text-muted">Loading tasks...</p>
                            ) : error ? (
                                <p className="text-danger">{error}</p>
                            ) : (
                                <TablaTasks data={tasks} />
                            )}
                        </CardBody>
                    </Card>
                </Col>
            </Row>

          
            <ModalTasks isOpen={isModalOpen} toggleModal={toggleModal} onModalClose={handlePageUpdate} updatePage={showTasks} />

        </Container>
    );
};

export default App;
